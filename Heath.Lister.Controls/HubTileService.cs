#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

#endregion

namespace Heath.Lister.Controls
{
    public static class HubTileService
    {
        private const int NumberOfSimultaneousAnimations = 1;
        private const int WaitingSteps = 6;

        private static readonly List<WeakReference> _enabledHubTiles = new List<WeakReference>();
        private static readonly Random _random = new Random();
        private static readonly DispatcherTimer _timer = new DispatcherTimer();
        private static readonly List<WeakReference> _stalledHubTiles = new List<WeakReference>();

        static HubTileService()
        {
            _timer.Tick += OnTimerTick;
        }

        public static void InitializeReference(HubTile tile)
        {
            var wref = new WeakReference(tile, false);

            AddReferenceToEnabledHubTiles(wref);
            RestartTimer();
        }

        internal static void FinalizeReference(HubTile tile)
        {
            var wref = new WeakReference(tile, false);

            RemoveReferenceFromEnabledHubTiles(wref);
            RemoveReferenceFromStalledHubTiles(wref);
        }

        private static void AddReferenceToEnabledHubTiles(WeakReference wref)
        {
            if (ContainsTarget(_enabledHubTiles, wref.Target))
                return;

            _enabledHubTiles.Add(wref);
        }

        private static void AddReferenceToStalledHubTiles(WeakReference wref)
        {
            if (ContainsTarget(_stalledHubTiles, wref.Target))
                return;

            _stalledHubTiles.Add(wref);
        }

        private static void RemoveReferenceFromEnabledHubTiles(WeakReference tile)
        {
            RemoveTarget(_enabledHubTiles, tile.Target);
        }

        private static void RemoveReferenceFromStalledHubTiles(WeakReference tile)
        {
            RemoveTarget(_stalledHubTiles, tile.Target);
        }

        private static bool ContainsTarget(IEnumerable<WeakReference> list, Object target)
        {
            return list.Any(t => t.Target == target);
        }

        private static void RemoveTarget(IList<WeakReference> list, Object target)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Target == target)
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        private static void RestartTimer()
        {
            if (_timer.IsEnabled)
                return;

            _timer.Interval = TimeSpan.FromMilliseconds(2500);
            _timer.Start();
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            for (var i = 0; i < _stalledHubTiles.Count; i++)
            {
                var stalledHubTileRef = _stalledHubTiles[i];

                if (((HubTile)stalledHubTileRef.Target).StallingCounter-- == 0)
                {
                    AddReferenceToEnabledHubTiles(stalledHubTileRef);
                    RemoveReferenceFromStalledHubTiles(stalledHubTileRef);
                    i--;
                }
            }

            if (_enabledHubTiles.Any())
            {
                for (var j = 0; j < NumberOfSimultaneousAnimations; j++)
                {
                    var index = _random.Next(_enabledHubTiles.Count);
                    var enabledHubTileRef = _enabledHubTiles[index];

                    switch (((HubTile)enabledHubTileRef.Target).State)
                    {
                        case HubTileState.Normal:
                            ((HubTile)enabledHubTileRef.Target).State = HubTileState.Flipped;
                            break;

                        case HubTileState.Flipped:
                            ((HubTile)enabledHubTileRef.Target).State = HubTileState.Normal;
                            break;
                    }

                    ((HubTile)enabledHubTileRef.Target).StallingCounter = WaitingSteps;
                    AddReferenceToStalledHubTiles(enabledHubTileRef);
                    RemoveReferenceFromEnabledHubTiles(enabledHubTileRef);
                }
            }

            else if (!_stalledHubTiles.Any())
                return;

            _timer.Interval = TimeSpan.FromMilliseconds(_random.Next(1, 31) * 100);
            _timer.Start();
        }
    }
}
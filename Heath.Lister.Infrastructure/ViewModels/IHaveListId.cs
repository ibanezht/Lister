#region usings

using System;

#endregion

namespace Heath.Lister.Infrastructure.ViewModels
{
    public interface IHaveListId : IHaveId
    {
        Guid ListId { get; set; }
    }
}
#region usings

using System;

#endregion

namespace Heath.Lister.Infrastructure.ViewModels
{
    public interface IHaveId //: IViewModel
    {
        Guid Id { get; set; }
    }
}
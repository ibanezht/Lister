#region usings

using System;

#endregion

namespace Heath.Lister.Infrastructure.ViewModels
{
    public interface IHaveId
    {
        Guid Id { get; set; }
    }
}
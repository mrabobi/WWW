using SmartHome.ThingAPI.Models;
using System;
using System.Collections.Generic;

namespace SmartHome.ThingAPI.Interfaces
{
    public interface ILocksService
    {
        public SmartLock AddLock(SmartLock lockModel);
        public bool DeleteLock(Guid lockId);
        public SmartLock UpdateLock(SmartLock lockModel);
        public List<SmartLock> GetLocksByOwner(Guid ownerId);
        public bool IsLockOwner(Guid lockId, Guid ownerId);
        public SmartLock GetById(Guid lockId);
        public bool LockExists(Guid lockId);
    }
}

using SmartHome.ThingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.ThingAPI.Interfaces
{
    public interface IBulbsService
    {
        public SmartBulb AddBulb(SmartBulb bulb);
        public bool DeleteBulb(Guid bulbId);
        public SmartBulb UpdateBulb(SmartBulb bulb);
        public List<SmartBulb> GetBulbsByOwner(Guid ownerId);
        public bool IsBulbOwner(Guid bulbId, Guid ownerId);
        public SmartBulb GetById(Guid bulbId);
        public bool BulbExists(Guid bulbId);
    }
}

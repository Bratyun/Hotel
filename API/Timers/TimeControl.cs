using API.DAL;
using Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Timers
{
    public class TimeControl
    {
        private readonly ApiCheck _checks;
        private readonly ApiRoom _rooms;

        public TimeControl(ApiRoom rooms, ApiCheck checks)
        {
            _rooms = rooms;
            _checks = checks;
        }

        public void CheckTimeOver(object sourse, System.Timers.ElapsedEventArgs e)
        {
            List<Check> checks = _checks.GetAll();
            if (checks == null)
            {
                return;
            }
            foreach (var item in checks)
            {
                if ((DateTime.Now.Date - item.RegisterDate.Date).TotalDays >= 2 && item.Status == CheckStatus.New)
                {
                    _rooms.SetRoomFree(item.RoomId);
                    item.Status = CheckStatus.Failed;
                    _checks.Update(item.Id, item);
                }
            }
        }
        
        internal void RoomTimeOver(object sourse, System.Timers.ElapsedEventArgs e)
        {
            List<Room> checks = _rooms.GetAll();
            if (checks != null)
            {
                foreach (var item in checks)
                {
                    if (item.EndDate.Date < DateTime.Now.Date)
                    {
                        _rooms.SetRoomFree(item.Id);
                    }
                }
            }

        }
    }
}

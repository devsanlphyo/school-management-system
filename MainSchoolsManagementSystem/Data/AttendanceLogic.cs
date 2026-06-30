using System;

namespace MainSchoolsManagementSystem.Data
{
    public static class AttendanceLogic
    {
        public static AttendanceStatus CalculateStatus(DateTime checkInTimeUtc, TimeSpan deadline)
        {
            var localCheckIn = checkInTimeUtc.ToLocalTime();
            return localCheckIn.TimeOfDay > deadline ? AttendanceStatus.Late : AttendanceStatus.Present;
        }
    }
}

using System;
using Xunit;
using MainSchoolsManagementSystem.Data;

namespace MainSchoolsManagementSystem.Tests
{
    public class AttendanceLogicTests
    {
        [Fact]
        public void CalculateStatus_ShouldReturnPresent_WhenBeforeDeadline()
        {
            // Arrange
            var deadline = new TimeSpan(8, 30, 0);
            
            // Construct a local time that is before the deadline (8:00 AM local)
            var localTime = new DateTime(2026, 6, 30, 8, 0, 0, DateTimeKind.Local);
            var checkInTimeUtc = localTime.ToUniversalTime();

            // Act
            var status = AttendanceLogic.CalculateStatus(checkInTimeUtc, deadline);

            // Assert
            Assert.Equal(AttendanceStatus.Present, status);
        }

        [Fact]
        public void CalculateStatus_ShouldReturnLate_WhenAfterDeadline()
        {
            // Arrange
            var deadline = new TimeSpan(8, 30, 0);
            
            // Construct a local time that is after the deadline (8:31 AM local)
            var localTime = new DateTime(2026, 6, 30, 8, 31, 0, DateTimeKind.Local);
            var checkInTimeUtc = localTime.ToUniversalTime();

            // Act
            var status = AttendanceLogic.CalculateStatus(checkInTimeUtc, deadline);

            // Assert
            Assert.Equal(AttendanceStatus.Late, status);
        }

        [Fact]
        public void CalculateStatus_ShouldReturnPresent_WhenExactlyOnDeadline()
        {
            // Arrange
            var deadline = new TimeSpan(8, 30, 0);
            
            // Construct a local time that is exactly on the deadline (8:30 AM local)
            var localTime = new DateTime(2026, 6, 30, 8, 30, 0, DateTimeKind.Local);
            var checkInTimeUtc = localTime.ToUniversalTime();

            // Act
            var status = AttendanceLogic.CalculateStatus(checkInTimeUtc, deadline);

            // Assert
            Assert.Equal(AttendanceStatus.Present, status);
        }
    }
}

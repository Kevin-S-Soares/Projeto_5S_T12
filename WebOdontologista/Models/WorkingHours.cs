using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Será implementada uma funcionalidade em uma versão futura.
 * Por enquanto esta classe nada faz.
 */
namespace WebOdontologista.Models
{
    public class WorkingHours
    {
        public int Id { get; set; }
        public TimeSpan StartingTime { get; set; }
        public TimeSpan EndingTime { get; set; }
        public TimeSpan LunchStartingTime { get; set; }
        public TimeSpan LunchEndingTime { get; set; }
        public byte DaysOfTheWeek { get; set; }
        public int AppointmentsDurationInMinutes { get; set; }
        public WorkingHours() { }
        public WorkingHours(TimeSpan startingTime, TimeSpan endingTime, TimeSpan lunchStartingTime, TimeSpan lunchEndingTime, byte daysOfTheWeek, int appointmentsDurationInMinutes)
        {
            StartingTime = startingTime;
            EndingTime = endingTime;
            LunchStartingTime = lunchStartingTime;
            LunchEndingTime = lunchEndingTime;
            DaysOfTheWeek = daysOfTheWeek;
            AppointmentsDurationInMinutes = appointmentsDurationInMinutes;
        }
    }
}

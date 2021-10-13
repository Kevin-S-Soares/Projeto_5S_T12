﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface IAssociativeTimePrototype
    {

        void Set(Dentist dentist);
        IAssociativeTimePrototype Clone();
        void MakeAppointment(Appointment appointment);
        void CancelAppointment(Appointment appointment);
        List<TimeSpan> AvailableTime(Appointment appointment);
        List<TimeSpan> EmptyList(Appointment appointment);

    }
}
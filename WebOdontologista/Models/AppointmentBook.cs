using System;
using System.Collections.Generic;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        public Dictionary<int, LinkedList<AppointmentList>> Book { get; private set; } = new Dictionary<int, LinkedList<AppointmentList>>();
        public AppointmentBook() { }
        public void AddDentist(int id)
        {
            Book.Add(id, new LinkedList<AppointmentList>());
        }
        public void AddAppointment(Appointment appointment)
        {
            LinkedListNode<AppointmentList> node;
            for (node = Book[appointment.DentistId].First; node != null; node = node.Next)
            {
                if (node.Value.SameDay(appointment.Date))
                {
                    try
                    {
                        node.Value.MakeAppointment(appointment);
                        break;
                    }
                    catch(DomainException e)
                    {
                        throw new DomainException(e.Message);
                    }
                }
                else if (node.Value.DayBefore(appointment.Date))
                {
                    LinkedListNode<AppointmentList> newNode = new LinkedListNode<AppointmentList>(new AppointmentList(appointment));
                    Book[appointment.DentistId].AddBefore(node, newNode);
                    break;
                }
            }
            if(node == null)
            {
                node = new LinkedListNode<AppointmentList>(new AppointmentList(appointment));
                Book[appointment.DentistId].AddLast(node);
            }
        }
        public void RemoveAppointment(Appointment appointment)
        {
            LinkedListNode<AppointmentList> node;
            for (node = Book[appointment.DentistId].First; node != null; node = node.Next)
            {
                if (node.Value.SameDay(appointment.Date))
                {
                    node.Value.CancelAppointment(appointment);
                    return;
                }
            }
            throw new DomainException("Consulta não encontrada!");
        }
        public List<TimeSpan> FindAvailableTime(Appointment appointment)
        {
            List<TimeSpan> result = null;
            LinkedListNode<AppointmentList> node;
            for (node = Book[appointment.DentistId].First; node != null; node = node.Next)
            {
                if (node.Value.SameDay(appointment.Date))
                {
                    result = node.Value.AvailableTime(appointment);
                    break;
                }
            }
            if(node == null)
            {
                result = AppointmentList.EmptyList(appointment);
            }
            return result;

        }

    }
}

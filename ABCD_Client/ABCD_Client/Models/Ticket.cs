//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ABCD_Client.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ticket()
        {
            this.Carts = new HashSet<Cart>();
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public int ticketId { get; set; }
        [DisplayName("Room ID")]
        public int roomId { get; set; }

        [DisplayName("Seat ID")]
        public int seatId { get; set; }

        [DisplayName("Seat Name")]
        public string seatName { get; set; }

        [DisplayName("Movie ID")]
        public int movieId { get; set; }

        [DisplayName("Screening ID")]
        public int screeningId { get; set; }

        [DisplayName("Ticket Code")]
        public string TicketCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual Movy Movy { get; set; }
        public virtual RoomSeat RoomSeat { get; set; }
        public virtual Screening Screening { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

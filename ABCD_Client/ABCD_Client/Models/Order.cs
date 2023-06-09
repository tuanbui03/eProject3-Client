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

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public int orderId { get; set; }
        public int customerId { get; set; }
        public Nullable<int> employeeId { get; set; }
        public Nullable<int> paymentId { get; set; }
        [DisplayName("Total Price")]
        public int totalPrice { get; set; }

        [DisplayName("Confirmed by Employee")]
        public bool isConfirm { get; set; }

        [DisplayName("Purchased")]
        public bool isPurchased { get; set; }

        [DisplayName("Booking Date")]
        public System.DateTime bookingDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}

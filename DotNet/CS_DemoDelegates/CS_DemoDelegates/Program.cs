using CS_DemoDelegates.PaymentsLib;

namespace CS_DemoDelegates
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PaymentService paymentService = new PaymentService();

            // Create delegates for each payment method
            PaymentMethod creditCardPayment = paymentService.CreditCardPayment;
            PaymentMethod payPalPayment = paymentService.PayPalPayment;
            PaymentMethod upiPayment = paymentService.UpiPayment;
            PaymentMethod netBankingPayment = paymentService.NetBankingPayment;

            // Invoke the delegates
            creditCardPayment?.Invoke(100.00m);
            payPalPayment?.Invoke(200.00m);
            upiPayment?.Invoke(300.00m);
            netBankingPayment?.Invoke(400.00m);

            // Send Notifications for each payment
            NotificationService notificationService = new NotificationService();
            Notify notify= notificationService.SendEmailNotification;
            notify += notificationService.SendSmsNotification; // Multicast delegate
            notify?.Invoke("Payment processed successfully.");


            Console.WriteLine("All payments processed.\nProgram completed. Press any key to exit.");
            Console.ReadKey();
        }
    }
}

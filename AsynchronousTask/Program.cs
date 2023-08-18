using System.Diagnostics;
using System.Xml.Linq;

namespace PurchaseOrder
{
    class Program
    {
      public  static void Main(string[] args)
        {

            Xmldata();
        }
        /// <summary>
        /// Method to read the XML Data 
        /// </summary>
        public static async void Xmldata()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var file = @"purchase_order.XML";
                var str = Path.GetFullPath(file);
                var xml = XDocument.Load(str);
                var order = xml.Descendants("PurchaseOrder");
                var orderId = order.Elements("ID").FirstOrDefault()!.Value;
                DateTime orderRequestDate = DateTime.Parse(order.Elements("RequestedShipDate").FirstOrDefault()!.Value);
                string createdBy = Convert.ToString(order.Elements("CreatedBy").FirstOrDefault()!.Value);
                int sumOfTheLoadFactor = 0;
                int quantityOfTheLoadFactor = 0;
                decimal TotalPrice = 0;
                var custmerInfo = order.Descendants("CustomerInfo");
                var Billing = custmerInfo.Descendants("Billing");
                string customerName = Convert.ToString(Billing.Elements("Name")!.FirstOrDefault()!.Value);
                string customerAddress = Convert.ToString(Billing.Elements("Address1").FirstOrDefault()!.Value);
                string customerPhone = Convert.ToString(Billing.Elements("Phone")!.FirstOrDefault()!.Value);
                string customerEmail = Convert.ToString(Billing.Elements("DeliveryReceiptEmail")!.FirstOrDefault()!.Value);

                List<int> manufacturingDay = new List<int>();
                foreach (var item in order.Descendants("Item"))
                {
                    var loadFactor = item.Element("LOADFACTOR")!.Value;
                    sumOfTheLoadFactor += int.Parse(loadFactor);
                    var quantity = int.Parse(item.Element("Order_Quantity")!.Value);
                    quantityOfTheLoadFactor += quantity;
                    var priceItem = decimal.Parse(item.Element("USPrice")!.Value);
                    TotalPrice += priceItem;
                    manufacturingDay.Add(int.Parse(item.Element("LeadTime")!.Value));
                }

                var availableTruck = await AvailableTruck(manufacturingDay.Max(), 1);
                Thread.Sleep(1000);

                Console.WriteLine("Available Truck: {0}", availableTruck != null ? availableTruck.TruckNumber + " Available Truck ResponseTime:" + stopwatch.Elapsed : " No Trucks Avaible in CurrentTime" + " Available Truck ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("OrderId: {0}", orderId + " OrderId ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("OrderRequestDate: {0}", orderRequestDate + " OrderRequestDate ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("CreatedBy: {0}", createdBy + " CreatedBy ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Load of the order: {0}", sumOfTheLoadFactor + " Load of the order ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Quantities of the Order: {0}", quantityOfTheLoadFactor + "  Quantities of the Order ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Price: {0}", TotalPrice + " Price:" + stopwatch.Elapsed);

                Console.WriteLine("Manufacturing Days: {0}", manufacturingDay.Max() + "  Manufacturing Days ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Customer Name: {0}", customerName + " Customer Name ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Customer Address: {0}", customerAddress + " Customer Address ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Customer Phone: {0}", customerPhone + " Customer Phone ResponseTime:" + stopwatch.Elapsed);

                Console.WriteLine("Customer Email: {0}, Customer Email ResponseTime: {1}", customerEmail, stopwatch.Elapsed);

                Console.WriteLine("Response Time: {0} milliseconds", stopwatch.ElapsedMilliseconds);

                stopwatch.Stop();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// AvailableTruck
        /// </summary>
        /// <param name="manufacturingDays"></param>
        /// <param name="Load"></param>
        /// <returns></returns>
        private static async Task<AvaibleTrucks> AvailableTruck(int manufacturingDays, int Load)
        {
            Thread.Sleep(1000);
            AvaibleTrucks Truck = null!;
            List<AvaibleTrucks> availableTrucks = new List<AvaibleTrucks>()
            {
               new AvaibleTrucks() { TruckNumber ="1234", TruckStartsOn = 8 , AvaibleWeight = 7  },
               new AvaibleTrucks() { TruckNumber ="1235", TruckStartsOn = 10 , AvaibleWeight = 35  },
               new AvaibleTrucks() { TruckNumber ="125", TruckStartsOn = 2 , AvaibleWeight = 10 },
               new AvaibleTrucks() { TruckNumber ="54202", TruckStartsOn = 3 , AvaibleWeight = 98  },
               new AvaibleTrucks() { TruckNumber ="5656", TruckStartsOn = 42 , AvaibleWeight = 24  },
               new AvaibleTrucks() { TruckNumber ="741", TruckStartsOn = 3, AvaibleWeight = 44  },
            };
            var availableTruck = availableTrucks.Where(x => x.AvaibleWeight > Load && x.TruckStartsOn > manufacturingDays).ToList();
            Truck = availableTruck.MinBy(x => x.TruckStartsOn)!;
            return Truck;
        }
    }

    /// <summary>
    /// AvaibleTrucks Class 
    /// </summary>
    public class AvaibleTrucks
    {
        public string TruckNumber { get; set; } = string.Empty;

        public int TruckStartsOn { get; set; }

        public int AvaibleWeight { get; set; }
    }
}

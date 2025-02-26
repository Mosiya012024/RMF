using Microsoft.AspNetCore.Mvc;
using ParkingLott.Enums;
using ParkingLott.Models;
using System.Linq;

namespace ParkingLott.Controllers
{
    public class ParkingLottController : Controller
    {
        public static readonly List<Lott> ParkingLotts = new List<Lott>()
        {
            new Lott()
            {
                floor_number = 7,
                Lott_Slots =
                {
                    Slots = new Slot[]
                    {
                        new Slot() { Slot_Id = Guid.NewGuid(), Slot_Type = LottType.Truck, slot_number = 67, Vehicle_Number=null, Status = Slot_Status.Available, InTime = null,OutTime=null},
                        new Slot() { Slot_Id = Guid.NewGuid(), Slot_Type = LottType.Bike, slot_number = 23, Vehicle_Number=null, Status = Slot_Status.Available, InTime = null,OutTime=null},
                        new Slot() { Slot_Id = Guid.NewGuid(), Slot_Type = LottType.Car, slot_number = 45, Vehicle_Number=null, Status = Slot_Status.Available, InTime = null,OutTime=null},
                    }
                }

            }
        };

        [HttpPost]
        [Route("/post/parkingLott")]
        public async Task<IActionResult> PostParkingLott(int Number_Of_Floors)
        {
            var lastFloorNumber = ParkingLotts.OrderByDescending(x => x.floor_number).First().floor_number;
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                List<int> SlotNumbers = new List<int>() { 101, 34, 54, 67, 76, 23, 78, 23, 89 ,132,456,4456,12,456,789,343,786,121,4345,6686};
                var i = 0;
                while (Number_Of_Floors >= 1)
                {
                    ParkingLotts.Add(
                    new Lott()
                    {
                        floor_number = lastFloorNumber++,
                        Lott_Slots =
                        {
                        Slots = new Slot[]
                        {
                            new Slot() { Slot_Id = Guid.NewGuid(), Slot_Type = LottType.Truck, slot_number = SlotNumbers[i], Vehicle_Number=null, Status = Slot_Status.Available, InTime = null,OutTime=null},
                            new Slot() { Slot_Id = Guid.NewGuid(), Slot_Type = LottType.Bike, slot_number = SlotNumbers[i+1], Vehicle_Number=null, Status = Slot_Status.Available, InTime = null,OutTime=null},
                            new Slot() { Slot_Id = Guid.NewGuid(), Slot_Type = LottType.Car, slot_number = SlotNumbers[i+2], Vehicle_Number=null, Status = Slot_Status.Available, InTime = null,OutTime=null},
                        }
                        }

                    });

                    Number_Of_Floors--;
                    i = i + 3;
                }

                result.Data = ParkingLotts;
                result.Code = ApiResponseCode.Success;
                result.Errors = new List<string>();
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Code = ApiResponseCode.Error;
                result.Errors = new List<string>() { e.Message};
                return BadRequest(result);
            }
        }

        [HttpPut]
        [Route("/park/vehicle")] 
        public async Task<IActionResult> parkVehicle([FromBody]Vehicle vehicle)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                //var FloorOrderedParkingLott = ParkingLotts.OrderBy(x => x.floor_number);
                //var allSlots = FloorOrderedParkingLott.Select(x => x.Lott_Slots.Slots);
                //var availableSlotts = allSlots!.First().OrderBy(x => x.slot_number);
                //var selectedSlot = availableSlotts.FirstOrDefault(x => x.Status == Slot_Status.Avaliable && x.Slot_Type == vehicle.Vehiicle_Type);
                //selectedSlot.InTime = DateTime.Now;
                //selectedSlot.Status = Slot_Status.Occupied;

                //var abcd = ParkingLotts.OrderBy(x => x.floor_number)
                //           .Select(x => x.Lott_Slots.Slots).First()
                //           .Where(x => x.Status == Slot_Status.Available && x.Slot_Type == vehicle.Vehiicle_Type)
                //           .OrderBy(x => x.slot_number).First();


                //if (abcd != null)
                //{
                //    abcd.InTime = DateTime.Now;
                //    abcd.Status = Slot_Status.Occupied;
                //    abcd.Vehicle_Number = vehicle.Vehicle_Number;
                //    result.Errors = new List<string>();
                //}
                //else
                //{
                //    List<string> errors = new List<string>();
                //    errors.Add("Parking Lott is fulll");
                //    result.Errors = errors;
                //}
                //result.Data = ParkingLotts;
                //result.Code = ApiResponseCode.Success;

                //return Ok(result);

                var abcd = ParkingLotts.OrderBy(x => x.floor_number);
                foreach(var i in abcd)
                {
                    var orderedSlots = i.Lott_Slots.Slots.OrderBy(x=>x.slot_number).ToList();
                    var abcde = orderedSlots.FirstOrDefault(x=>x.Status == Slot_Status.Available && x.Slot_Type == vehicle.Vehiicle_Type);
                    if(abcde != null)
                    {
                        abcde.InTime = DateTime.Now;
                        abcde.Status = Slot_Status.Occupied;
                        abcde.Vehicle_Number = vehicle.Vehicle_Number;
                        result.Errors = new List<string>();
                        result.Ticket_ID = i.Lott_Id + " " + abcde.Slot_Id + " " + i.floor_number;
                        break;
                    }
                }

                result.Data = ParkingLotts;
                result.Code = ApiResponseCode.Success;

                return Ok(result);


            }
            catch (Exception e)
            {
                result.Data = null;
                result.Code = ApiResponseCode.Error;
                result.Errors = new List<string>() { e.Message };
                return BadRequest(result);
            }

        }

        [HttpPut]
        [Route("/Unpark/vehicle")]
        public async Task<IActionResult> UnParkVehicle(Vehicle vehicle)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var abcde = ParkingLotts.SelectMany(x=>x.Lott_Slots.Slots).First(x=>x.Vehicle_Number == vehicle.Vehicle_Number);
                if (abcde == null)
                {
                    result.Data = "Invalid Ticket";
                    result.Code = ApiResponseCode.Success;
                    return Ok(result);
                }
                else
                {


                    abcde.OutTime = DateTime.Now;
                    abcde.Status = Slot_Status.Available;
                    result.Data = abcde;
                    result.Code = ApiResponseCode.Success;
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                result.Data = null;
                result.Code = ApiResponseCode.Error;
                result.Errors = new List<string>() { e.Message };
                return BadRequest(result);
            }
        }

        [HttpGet]
        [Route("/count/availableslots/floor")]
        public async Task<IActionResult> GetAvailableSlotsCount(int floor_no)
        {
            var count = ParkingLotts.First(x=>x.floor_number == floor_no).Lott_Slots.Slots.Where(x=>x.Status == Slot_Status.Available).Count();
            return Ok(count);
        }

        [HttpGet]
        [Route("/display/availableslots/floor")]
        public async Task<IActionResult> DisplayAvailableSlots(int floor_no)
        {
            var abcdef = ParkingLotts.First(x => x.floor_number == floor_no).Lott_Slots.Slots.Where(x => x.Status == Slot_Status.Available);
            return Ok(abcdef);
        }

        [HttpGet]
        [Route("/display/occupiedslots/VehicleType")]
        public async Task<IActionResult> DisplayOccupiedSlotsVehicleType(LottType type)
        {
            var bcde = ParkingLotts.SelectMany(x => x.Lott_Slots.Slots).Where(x => x.Status == Slot_Status.Occupied && x.Slot_Type == type);
            return Ok(bcde);
        }


        [HttpGet]
        [Route("/count/availableslots/perfloor")]
        public async Task<IActionResult> GetAvailableSlotsCountPerFloor(LottType type)
        {
            List<string> list = new List<string>();
            foreach(var i in ParkingLotts.OrderBy(x => x.floor_number))
            {
                var count = i.Lott_Slots.Slots.Count(x => x.Status == Slot_Status.Available && x.Slot_Type == type);
                list.Add("Count of " + type.ToString() + " in " + i.floor_number + " floor - " +count);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("/display/availableslots/perfloor")]
        public async Task<IActionResult> DisplayAvailableSlotsPerFloor(LottType type)
        {
            List<Slot> list = new List<Slot>();
            foreach (var i in ParkingLotts.OrderBy(x => x.floor_number))
            {
                var data = i.Lott_Slots.Slots.Where(x => x.Status == Slot_Status.Available && x.Slot_Type == type) ?? new List<Slot>();
                list.AddRange(data);
                //list.Add("Count of " + type.ToString() + " in " + i.floor_number + " floor - " + count);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("/display/occupiedslots/perfloor")]
        public async Task<IActionResult> DisplayOccupiedSlotsPerFloor(LottType type)
        {
            List<string> list = new List<string>();
            foreach (var i in ParkingLotts.OrderBy(x => x.floor_number))
            {
                var count = i.Lott_Slots.Slots.Count(x => x.Status == Slot_Status.Occupied && x.Slot_Type == type);
                list.Add("Count of " + type.ToString() + " in " + i.floor_number + " floor - " + count);
            }
            Dictionary<int, string> name_dict = new Dictionary<int, string>();
            name_dict.Add(1, "mosiya");
            name_dict.Add(2, "kabeer");
            name_dict.Add(3, "Zaara");
            name_dict.Add(4, "Imran");
            name_dict.Add(5, "Reema");
            if(name_dict.TryGetValue(2,out var user_name))
            {
                Console.WriteLine(user_name);
            }
            else
            {
                Console.WriteLine("value not found in dictionary");
            }
            if(Enum.TryParse("1",true, out LottType lott_type))
            {
                Console.WriteLine(lott_type);
            }
            else
            {
                Console.WriteLine("value not found in Enum");
            }
            if (Enum.TryParse("Bike", true, out LottType lott_type2))
            {
                Console.WriteLine(lott_type2);
            }
            else
            {
                Console.WriteLine("value not found in Enum");
            }
            return Ok(list);
        }


    }
}

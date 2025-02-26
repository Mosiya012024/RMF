using Microsoft.AspNetCore.Mvc;
using ParkingLott.Interface;
using ParkingLott.Models2;

namespace ParkingLott.Services
{
    public class LibraryRepositoryDupe : LibraryRepository
    {
        public static readonly Library libs = new Library();
        public async override Task<Library> CreateLibrary()
        {
            libs.Library_Id = Guid.NewGuid();
            libs.Racks = new List<Rack>();
            libs.Racks.Add(
                new Rack(true)
            {
                Rack_Id = Guid.NewGuid(),
                Book_Info = {
                    Title = "Meri Mout",
                    Authors = "Mosiya",
                    Publishers = "Mera janaaza uthaane waale"
                },
                RStatus = Enum2.RackStatus.Occupied,
            }) ;
            return libs;
        }

        
    }
}

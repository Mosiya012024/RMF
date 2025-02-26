using Microsoft.AspNetCore.Mvc;
using ParkingLott.Models2;

namespace ParkingLott.Interface
{
    public interface ILibraryRepository
    {
        public Task<Library> CreateLibrary();

        public Task<Library> GetLibrary();

        public Task<Rack> UpdateLibrary(Guid UserID,Guid BookID);

        public void DeleteLibrary();

    }
}

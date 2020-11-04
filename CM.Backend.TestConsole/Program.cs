using MongoDB.Bson;

namespace CM.Backend.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var match = BCrypt.BCryptHelper.CheckPassword("dlk11j5u17", "$2a$10$NUH76rvReKqjdm/cCb.wVeQ16OoTmKaJR5iV/sfzY.ASi2c9F5Tl2");
            
            var id = ObjectId.GenerateNewId().ToString();
        }
    }
}

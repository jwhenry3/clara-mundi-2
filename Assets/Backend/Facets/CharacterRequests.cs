using System;

namespace Backend.App
{
    [Serializable]
    public class CreateCharacterRequest
    {
        public string Name;
        public string Gender;
        public string Race;
        
        // TBD: add character customization options to the request
    }
}
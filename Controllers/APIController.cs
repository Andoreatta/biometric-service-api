using Microsoft.AspNetCore.Mvc;

namespace BiometricService.Controllers
{
    [ApiController]
    [Route("apiservice/")]
    public class APIController : ControllerBase
    {
        private readonly Biometric _biometric;

        public APIController(Biometric biometric)
        {
            _biometric = biometric;
        }

        [HttpGet("capture-hash")]
        public string Capture()
        {
            return _biometric.CaptureHash();
        }

        [HttpGet("identification")]
        public string Identification()
        {
            return _biometric.Identification();
        }

        [HttpPost("load-to-memory")]
        public string LoadToMemory([FromBody] Finger[] fingers)
        {
            return _biometric.LoadToMemory(fingers);
        }

        [HttpGet("delete-all-from-memory")]
        public string DeleteAllFromMemory()
        {
            return _biometric.DeleteAllFromMemory();
        }
    }
}
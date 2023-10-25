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

        [HttpPost("match-one-on-one")]
        public bool MatchOneOnOne([FromBody] string fingerprintHash)
        {
            return _biometric.IdentifyOneOnOne(fingerprintHash);
        }

        [HttpGet("identification")]
        public uint Identification()
        {
            return _biometric.Identification();
        }

        [HttpPost("load-to-memory")]
        public bool LoadToMemory([FromBody] Finger[] fingers)
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
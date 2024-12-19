using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Scaler.Core.Services;
using Scaler.Core.Services.Shop;
using Scaler.Server.ViewModels.Shop;

namespace Scaler.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(
        IMapper mapper,
        ILogger<CustomerController> logger,
        IEmailSender emailSender,
        ICustomerService customerService)
        : ControllerBase
    {
        private readonly ILogger _logger = logger;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var allCustomers = customerService.GetAllCustomersData();
            return Ok(mapper.Map<IEnumerable<CustomerVm>>(allCustomers));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CustomerException"></exception>
        [HttpGet("throw")]
        public IEnumerable<CustomerVm> Throw()
        {
            throw new CustomerException($"This is a test exception: {DateTime.Now}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("email")]
        public async Task<string> Email()
        {
            var senderName = "Sender Name";
            var senderEmail = "Sender Email";
            var recipientName = "Scaler recipientName";
            var recipientEmail = "recipient@scaler.com";
            var subject = "Subject test";
            var body = "body Test !";
            string result;
            try
            {
                await emailSender.SendEmailAsync(senderName, senderEmail, recipientName, recipientEmail, subject, body);

                result = "Success";
            }
            catch (Exception ex)
            {
                result = $"Error: {ex.Message}";
            }

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"value: {id}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

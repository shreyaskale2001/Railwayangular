
using RailwayReservationJWT.Data;
using RailwayReservationJWT.Models;
using RailwayReservationJWT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Management.Service.Services;
using User.Management.Service.Model;
using System;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace RailwayReservationJWT.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]

    public class TicketController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RailwayContext context;
        private readonly IEmailService _emailService;
        //private object resultList;

        public TicketController(UserManager<IdentityUser> userManager, RailwayContext context, IEmailService emailService)
        {
            this._userManager = userManager;
            this.context = context;
            _emailService = emailService;

        }
        [HttpPost]
        //[Route("Booking")]
        public async Task<IActionResult> Booking(TicketData ticketData)
        {
            string userId = "544ea7ff-ceae-4677-89dd-282f162ecd40";

            if (userId != null)
            {
                int TId = ticketData.TrainNo;
                TrainDetail trainDetail = context.trainDetails.FirstOrDefault(id => id.TrainNo == TId);
                Ticket ticket = new Ticket();
                ticket.UserName = ticketData.UserName;
                ticket.Age = ticketData.Age;
                ticket.Gender = ticketData.Gender;
                ticket.TrainNo = TId;
                ticket.Passenger = ticketData.Passenger;

                if (ticketData.TicketType == "SL" && trainDetail.SeatCount_Slepper > 0)
                {
                    ticket.SeatNo = "SL" + (trainDetail.SeatCount_Slepper - trainDetail.SeatCount_Slepper + 1);
                    trainDetail.SeatCount_Slepper -= ticketData.Passenger;
                }
                else if (ticketData.TicketType == "AC1" && trainDetail.SeatCount_AC1tire > 0)
                {
                    ticket.SeatNo = "AC1" + (trainDetail.SeatCount_AC1tire - trainDetail.SeatCount_AC1tire + 1);
                    trainDetail.SeatCount_AC1tire -= ticketData.Passenger;
                }
                else if (ticketData.TicketType == "AC2" && trainDetail.SeatCount_AC2tire > 0)
                {
                    ticket.SeatNo = "AC2" + (trainDetail.SeatCount_AC2tire - trainDetail.SeatCount_AC2tire + 1);
                    trainDetail.SeatCount_AC2tire -= ticketData.Passenger;
                }
                else if (ticketData.TicketType == "AC3" && trainDetail.SeatCount_AC3tire > 0)
                {
                    ticket.SeatNo = "AC3" + (trainDetail.SeatCount_AC3tire - trainDetail.SeatCount_AC3tire + 1);
                    trainDetail.SeatCount_AC3tire -= ticketData.Passenger;
                }
                else
                {
                    ticket.SeatNo = "G" + (trainDetail.SeatCount_SecoundSetting - trainDetail.SeatCount_SecoundSetting + 1);
                    trainDetail.SeatCount_SecoundSetting -= ticketData.Passenger;
                }

                context.tickets.Add(ticket);
                context.SaveChanges();

                if (ticket.TicketNo != 0)
                {
                    var message = new Message(new string[] { "shreyaskale.ssk@gmail.com" }, "Booking confirmation",
                        "Your booking is succesfully done!\nBooking ID: " + ticket.TicketNo +
                        "\nUser Name. " + ticket.UserName +
                        "\nJourney from: " + trainDetail.ArrivalLocation +
                        "\nJourney to: " + trainDetail.DestinationLocation +
                        "\nJourney time: " + trainDetail.JourneyTime +
                        "\nJourney Date :" + trainDetail.StartDate +
                        "\nTrain name: " + trainDetail.TrainName +
                        "\nTicket Type :" + ticketData.TicketType +
                        "\nNo of Passenger :" + ticketData.Passenger

                        );

                    _emailService.SendEmail(message);

                    return Ok(new
                    {
                        status = "success",
                        message = "booking successfull",
                        
                    });
                }
            }
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Unable to book the ticket", Message = "Please try again!" });
                }
            
            //[HttpGet("id")]
            //public async Task<ActionResult<Ticket>> GetTicketDetail(int id)
            //{
            //    if (context.tickets == null)
            //    {
            //        return NotFound();
            //    }
            //    var ticket = await context.tickets.FindAsync(id);
            //    if (ticket == null)
            //    {
            //        return NotFound();
            //    }
            //    return ticket;
            //}


            //[HttpDelete("{id}")]
            //public async Task<IActionResult> DeleteTicket(int id)
            //{
            //    if (context.tickets == null)
            //    {
            //        return NotFound();
            //    }
            //    var ticket = await context.tickets.FindAsync(id);
            //    if (ticket == null)
            //    {
            //        return NotFound();
            //    }
            //    context.tickets.Remove(ticket);
            //    await context.SaveChangesAsync();
            //    return Ok();
            //}

        }
    }


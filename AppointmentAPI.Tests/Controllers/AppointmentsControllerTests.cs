using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentManager.Controllers;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Xunit;

namespace AppointmentAPI.Tests.Controllers
{
    public class AppointmentsControllerTests
    {
        private readonly AppointmentDbContext _context;
        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            _context = TestDbContext.GetTestDbContext();
            _controller = new AppointmentsController(_context);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreatedAtAction()
        {
            // Arrange
            var appointment = new Appointment
            {
                Title = "Test Appointment",
                Date = DateTime.UtcNow.AddDays(1), // Future date
                User = "user123",
                IsApproved = false,
                IsCanceled = false
            };

            // Act
            var result = await _controller.CreateAppointment(appointment);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdAppointment = Assert.IsType<Appointment>(actionResult.Value);
            Assert.Equal("Test Appointment", createdAppointment.Title);
        }

        [Fact]
        public async Task GetUserAppointments_ReturnsAppointments()
        {
            // Arrange
            var testUser = "user123";
            var appointment = new Appointment
            {
                Title = "Doctor Appointment",
                Date = DateTime.UtcNow.AddDays(2),
                User = testUser,
                IsApproved = false,
                IsCanceled = false
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetUserAppointments(testUser);

            // Assert
            var appointments = Assert.IsType<List<Appointment>>(result.Value);
            Assert.Single(appointments);
            Assert.Equal("Doctor Appointment", appointments.First().Title);
        }

        [Fact]
        public async Task DeleteAppointment_ReturnsNoContent()
        {
            // Arrange
            var appointment = new Appointment
            {
                Title = "Meeting",
                Date = DateTime.UtcNow.AddDays(1),
                User = "user123",
                IsApproved = false,
                IsCanceled = false
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteAppointment(appointment.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

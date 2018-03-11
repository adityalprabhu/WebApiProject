using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData;
using ContactDiary.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ContactDiary.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly ContactContext _context;

        private ContactItem contact = new ContactItem
        {
            Name = "newName",
            Number = "445454545",
            Email = "test@test12.com"
        };

        public ContactController(ContactContext context)
        {
            _context = context;

            if (_context.ContactItems.Count() == 0)
            {
                _context.ContactItems.Add(new ContactItem { Name = "test", Email = "adityalprabhu@gmail.com", Number = "9876543210" });
                _context.SaveChanges();
            }
        }


        //gets all the contacts saved
        [HttpGet]
        public IEnumerable<ContactItem> GetAll()
        {
            return _context.ContactItems.ToList();
        }

        //gets first contact by the name given
        [HttpGet("byName/{name}", Name = "GetContactByName")]
        public IActionResult GetByName(string name)
        {
            
            var item = _context.ContactItems.FirstOrDefault(t => t.Name == name);
            if (IsEmpty(item))
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }


        //gets the contact by id given
        [HttpGet("{id}", Name = "GetContactById")]
        public IActionResult GetById(long id)
        {
            var item = _context.ContactItems.FirstOrDefault(t => t.Id == id);
            if (IsEmpty(item))
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        //posts a new contact
        [HttpPost]
        public IActionResult Create([FromBody] ContactItem item)
        {
            if (IsEmpty(item) || IsEmpty(item.Name) || (IsEmpty(item.Number) && IsEmpty(item.Email)))
            {
                return BadRequest();
            }

            _context.ContactItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetContactById", new { id = item.Id }, item);
        }

        //updating a contact item
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] ContactItem item)
        {
            if (IsEmpty(item) || item.Id != id)
            {
                return BadRequest();
            }

            var contact = _context.ContactItems.FirstOrDefault(t => t.Id == id);
            if (IsEmpty(contact))
            {
                return NotFound();
            }

            contact.Name = item.Name;
            contact.Number = item.Number;
            contact.Email = item.Email;

            _context.ContactItems.Update(contact);
            _context.SaveChanges();

            return new NoContentResult();
        }

        //deleting a contact item
        [HttpDelete("{id}")]
        public string Delete(long id)
        {
            var contact = _context.ContactItems.FirstOrDefault(t => t.Id == id);
            if (contact == null)
            {
                return "Not found!";
            }

            _context.ContactItems.Remove(contact);
            _context.SaveChanges();
            return "Sucessfully Deleted!";
        }

        //updating only a part of a contact item
        [HttpPatch("{id}")]
        public IActionResult Patch(long id, [FromBody]JsonPatchDocument<ContactItem> patchDocument)
        {
            if (IsEmpty(patchDocument))
            {
                return BadRequest();
            }

            var patchContact = _context.ContactItems.FirstOrDefault(t => t.Id == id);

            patchDocument.ApplyTo(patchContact, ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var model = new
            {
                original = _context.ContactItems.FirstOrDefault(t => t.Id == id),
                patched = patchContact
            };

            _context.ContactItems.Update(patchContact);
            _context.SaveChanges();

            return Ok(model);
        }






        public Boolean IsEmpty(Object item)
        {
           return (item == null);
        }
    }
}

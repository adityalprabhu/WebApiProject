using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData;
using ContactDiary.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ContactDiary.Controllers
{
    /// <summary>
    /// Contact controller. This class is a controller that 
    /// defines the methods for all REST calls 
    /// </summary>
    /// <remarks>
    /// This class has - GET (id, name, all), POST, PUT,
    /// PATCH and  DELETE methods
    /// </remarks>
    [Route("api/[controller]")]
    public class ContactController : Controller
    {

        //This is the context for the InMemory DB
        private readonly ContactContext _context;

        /* The database context is injected in the controller
         * using dependency injection
        */
        public ContactController(ContactContext context)
        {   
            //This context is used for all operations on the database
            _context = context;

            /*if there exist no elements in the db 
             * three new contacts are added to the db
            */
            if (_context.ContactItems.Count() == 0)
            {
                _context.ContactItems.Add(new ContactItem { Name = "John", Email = "john@test.com", Number = "9876543210" });
                _context.ContactItems.Add(new ContactItem { Name = "Jane", Email = "jane@test.com", Number = "6173838383" });
                _context.ContactItems.Add(new ContactItem { Name = "Jack", Email = "jack@test.com", Number = "8578339399" });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// GET - gets all the contacts in the db
        /// </summary>
        /// <returns>
        /// Returns all the contacts available
        /// Returns 404 - NotFound() if no contacs are 
        /// available in the db
        /// </returns>
        [HttpGet]
        public ICollection<ContactItem> GetAll()
        {
            return _context.ContactItems.ToList();
        }


        /// <summary>
        /// GET - gets the first contact with the given name
        /// </summary>
        /// <returns>
        /// Returns the first contact found with the given name
        /// Returns 404 - NotFound() if no contact with the given name
        /// is found
        /// </returns>
        [HttpGet("byName/{name}", Name = "GetContactByName")]
        public IActionResult GetByName(string name)
        {
            //returns the first occurrence of the contact with the given name
            var item = _context.ContactItems.FirstOrDefault(t => t.Name == name);
         
            if (IsEmpty(item))
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }


        /// <summary>
        /// GET - gets the first contact with the given id
        /// </summary>
        /// <returns>
        /// Returns the first contact found with the given id
        /// Returns 404 - NotFound() if no contact with the given id
        /// exists in the db
        /// </returns>
        [HttpGet("{id}", Name = "GetContactById")]
        public IActionResult GetById(long id)
        {
            //returns the first occurrence of the contact with the given id
            var item = _context.ContactItems.FirstOrDefault(t => t.Id == id);

            if (IsEmpty(item))
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        /// <summary>
        /// POST - creates the given contact in the DB
        /// </summary>
        /// <returns>
        /// Returns the newly created contact
        /// Returns BadRequest() if the format of the request
        /// is not correct
        /// </returns>
        [HttpPost]
        public IActionResult Create([FromBody] ContactItem item)
        {
            //Name or both number and email cannot be null
            if (IsEmpty(item) || IsEmpty(item.Name) || (IsEmpty(item.Number) && IsEmpty(item.Email)))
            {
                return BadRequest();
            }

            _context.ContactItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetContactById", new { id = item.Id }, item);
        }

 

        /// <summary>
        /// PUT - Updates an already existing contact.
        /// Entire contact item has to be sent for the
        /// existing item to be updated, otherwise the fields
        /// that aren't sent are set to NULL
        /// </summary>
        /// <returns>
        /// Returns the updated contact
        /// Returns 400 - BadRequest() if the format of the 
        /// contact is not correct
        /// Returns 404 - NotFound() if the given contact to be 
        /// updated is not found in the DB
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] ContactItem item)
        {
            if (IsEmpty(item) || item.Id != id)
            {
                return BadRequest();
            }

            var contactItem = _context.ContactItems.FirstOrDefault(t => t.Id == id);
            if (IsEmpty(contactItem))
            {
                return NotFound();
            }

            contactItem.Name = item.Name;
            contactItem.Number = item.Number;
            contactItem.Email = item.Email;

            _context.ContactItems.Update(contactItem);
            _context.SaveChanges();

            return Ok(contactItem);
        }

        /// <summary>
        /// DELETE - deletes an existing contact item from DB
        /// </summary>
        /// <returns>
        /// Returns a Success Message if deleted
        /// Returns 400 - BadRequest() if the format of the 
        /// given request is not correct
        /// Returns 404 - NotFound() if the given contact to be 
        /// deleted is not found in the DB
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var contactItem = _context.ContactItems.FirstOrDefault(t => t.Id == id);
            if (contactItem == null)
            {
                return NotFound();
            }

            _context.ContactItems.Remove(contactItem);
            _context.SaveChanges();

            return Ok("Contact Successfully Deleted!");
        }

        /// <summary>
        /// PATCH - partially updates an existing contact item.
        /// Only a part of the contact item can be updated.
        /// </summary>
        /// <returns>
        /// Returns a Success Message if contact is updated
        /// Returns 400 - BadRequest() if the format of the 
        /// given request is not correct
        /// Returns 404 - NotFound() if the given contact to be 
        /// updated is not found in the DB
        /// </returns>
        [HttpPatch("{id}")]
        public IActionResult Patch(long id, [FromBody]JsonPatchDocument<ContactItem> patchDocument)
        {
            if (IsEmpty(patchDocument))
            {
                return BadRequest();
            }

            //fetch the contact to be updated
            var patchContact = _context.ContactItems.FirstOrDefault(t => t.Id == id);

            if (patchContact == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(patchContact, ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _context.ContactItems.Update(patchContact);
            _context.SaveChanges();

            return Ok(patchContact);
        }

        /// <summary>
        /// IsEmpty is a helper method
        /// that checks if the given item is null
        /// </summary>
        /// <returns>
        /// Returns true if item is NULL, 
        /// else true if item is not NULL
        /// </returns>
        public Boolean IsEmpty(Object item)
        {
           return (item == null);
        }
    }
}

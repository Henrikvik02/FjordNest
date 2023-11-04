using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FjordNestPro.Models;
using FjordNestPro.Data;
using FjordNestPro.Repositories;

namespace FjordNestPro.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        // GET: Question
        public async Task<IActionResult> Index()
        {
            var questions = await _questionRepository.GetQuestionsAsync();
            return questions != null ? View(questions) : Problem("Entity set 'FjordNestProDbContext.Questions' is null.");

        }

        // GET: Question/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Question/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionID,Email,Content,ImageUrl,AnswerContent,AnswerImageUrl")] Question question)
        {
            if (ModelState.IsValid)
            {
                await _questionRepository.AddAsync(question);
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }



        private bool QuestionExists(int id)
        {
            return (_questionRepository.QuestionExists(id));
        }

        public IActionResult Help()
        {
            return View();
        }
        public IActionResult SendQuestion()
        {
            return View();
        }


        public async Task<IActionResult> QuestionList()
        {
            var questions = await _questionRepository.GetQuestionsAsync();
            return questions != null ? View(questions) : Problem("Entity set 'FjordNestProDbContext.Questions' is null.");
        }


        //
        //
        //
        //ALT UNDER SLETTES
        //
        //
        //

        // GET: Question/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionRepository.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // POST: Question/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionID,Email,Content,ImageUrl,AnswerContent,AnswerImageUrl")] Question question)
        {
            if (id != question.QuestionID)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _questionRepository.UpdateAsync(question);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: Question/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionRepository.GetQuestionByIdAsync(id.Value);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return Problem("Entity set 'FjordNestProDbContext.Questions'  is null.");
            }
       
            if (question != null)
            {
                await _questionRepository.DeleteAsync(question);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Question/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionRepository.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }
    }
}
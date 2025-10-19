using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageDemo.Models;

namespace RazorPageDemo.Pages
{
    public class CalculatorModel : PageModel
    {
        [BindProperty]
        public RazorPageDemo.Models.CalculatorModel Calc { get; set; } = new();

        public void OnGet()
        {
        }

        public void OnPostAdd()
        {
            Calc.Result = Calc.Number1 + Calc.Number2;
            Calc.Operation = "Addition";
        }

        public void OnPostSubtract()
        {
            Calc.Result = Calc.Number1 - Calc.Number2;
            Calc.Operation = "Subtraction";
        }

        public void OnPostMultiply()
        {
            Calc.Result = Calc.Number1 * Calc.Number2;
            Calc.Operation = "Multiplication";
        }

        public void OnPostDivide()
        {
            if (Calc.Number2 != 0)
            {
                Calc.Result = Calc.Number1 / Calc.Number2;
                Calc.Operation = "Division";
            }
            else
            {
                ModelState.AddModelError("Calc.Number2", "Cannot divide by zero!");
            }
        }
    }
}
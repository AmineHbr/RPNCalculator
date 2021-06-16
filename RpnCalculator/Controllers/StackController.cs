using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RpnCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StackController : ControllerBase
    {
        private static string[] operations = { "sum", "substract", "multiply", "divide" };
        private static Dictionary<int, Stack<int>> mystacks = new Dictionary<int, Stack<int>>();

        private readonly ILogger<StackController> _logger;

        public StackController(ILogger<StackController> logger)
        {
            _logger = logger;
        }


        //list All the operations
        [HttpGet("Operations")]
        public ActionResult GetAllOperations()
        {
            return Ok(operations);
        }

        //create a new Stack
        [HttpPost("CreateNewStack")]
        public ActionResult CreateNewStack()
        {
            //get the next id of the stack;
            var nextid = mystacks.Count > 0 ? mystacks.Keys.Max() + 1 : 1;
            mystacks.Add(nextid, new Stack<int>());
            return Ok("stack created with success it has the id :" + nextid);
        }

        //delete the a stack
        [HttpDelete("DeleteStack{idStack}")]
        public ActionResult DeleteStack(int idStack)
        {
            if (mystacks.ContainsKey(idStack))
            {
                mystacks.Remove(idStack);
                return Ok("removed the stack with the id :" + idStack);
            }
            return NotFound("the stack doesn't exist");
        }

        // get All the stacks
        [HttpGet("ListStack")]
        public ActionResult GetAllStacks()
        {
            return Ok(mystacks.Values);
        }
        // push a value to the stack
        [HttpPost("AddValue/Stack/{idStack}/Value/{value}")]
        public ActionResult AddValueTostack(int idStack,  int value)
        {
            //check if the stack exists
            if (mystacks.ContainsKey(idStack))
            {
                var stackwanted = mystacks.Where(x => x.Key == idStack).FirstOrDefault();
                stackwanted.Value.Push(value);
                return Ok(stackwanted);
            }
            return NotFound("the stack doesn't exist");
        }
        //get a specific stack
        [HttpGet("GetStack{idStack}")]
        public ActionResult GetStackWithId(int idStack)
        {
            //check if the stack exists
            if (mystacks.ContainsKey(idStack))
            {
                var stackwanted = mystacks.Where(x => x.Key == idStack).FirstOrDefault();
                return Ok(stackwanted);
            }
            return NotFound("the stack doesn't exist");
        }

        //Apply an opperand to the stack
        [HttpPost("ApplyOperand/{operation}/stack/{idStack}")]
        public ActionResult ApplyOperandOnStack(string operation, int idStack)
        {
            // check if the operand exists
           var myoperationindex = Array.IndexOf(operations, operation);
            if (myoperationindex < 0 )
                return NotFound("the Operation doesn't exist");

            //check if the stack exists
            if (!mystacks.ContainsKey(idStack))
                return NotFound("the stack doesn't exist");

            var stackwanted = mystacks.Where(x => x.Key == idStack).FirstOrDefault().Value;

            //check the is more than one value
            if (stackwanted.Count < 2) return Ok(stackwanted);

            //get the 2 top values
            var topvalue = stackwanted.Pop();
            var secondvalue= stackwanted.Pop();

            //apply the operand and push it in the stack
            switch (myoperationindex)
            {
                case 0:
                    stackwanted.Push(topvalue + secondvalue);
                    break;
                case 1:
                    stackwanted.Push(topvalue - secondvalue);
                    break;
                case 2:
                    stackwanted.Push(topvalue * secondvalue);
                    break;
                case 3:
                    if(secondvalue==0) return NotFound("division par zero");
                    stackwanted.Push(topvalue / secondvalue);
                    break;
                
             }
          

            return Ok(stackwanted);
        }
    }
}

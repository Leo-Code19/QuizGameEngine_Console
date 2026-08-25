using QuizGameEngine_1_.Models;
using QuizGameEngine_1_.Repos;
using QuizGameEngine_1_.Services;

namespace Program
{
    public class main
    { 
        public static void Main()
        {

            Console.SetWindowSize(80, 40);
            MenuService menuService = new MenuService();
            menuService.ShowMenu();

        }   
    }

}

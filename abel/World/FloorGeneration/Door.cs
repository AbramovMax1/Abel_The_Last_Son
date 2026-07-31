namespace ConsoleApp1;
/*
 * keynotes
 * positions
 * 0 = up
 * 1 = right
 * 2 = down
 * 3 = left
 */


public class Door
{
    int position;
    bool open;
    bool closed;

    public Door(int position)
    {
        this.position = position;
    }
}
class AnimatedTextureDummy : Sprite
{
    static public List<Sprite> Container = new List<Sprite>();
    public AnimatedTexture animtexture;
    public void ChangeTexture(string val)
    {
        animtexture.SetActive(val);
        Texture = animtexture.NextTexture();
    }
    public void AdvanceTexture()
    {
        Texture = animtexture.NextTexture();
    }
}
class Entity : AnimatedTextureDummy
{
    public Timer abc = new Timer(10, true, true);
    public Entity(Vector2f pos, AnimatedTexture animtexset)
    {
        Position = pos; animtexture = animtexset;
        abc.beep += TEXTURETIMER;
        abc.change(animtexture.GetDuration(), true);
    }
    public void TEXTURETIMER(object sender, EventArgs @e)
    {
        AdvanceTexture();
        abc.change(animtexture.GetDuration(), true);
    }
}
class Program
{
    static RenderWindow window = new RenderWindow(new VideoMode(600, 400), "abc", Styles.Default);
    static void Main()
    {
        Clock deltaclock = new Clock();
        while(window.IsOpen)
        {
            foreach(Timer tim in Timer.ActiveTimers)
            {
                tim.update(deltaclock.Restart());
            }
        }
    }
}

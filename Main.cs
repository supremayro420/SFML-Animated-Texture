using System.Collections.Generic;

class Timer
{
    public static List<Timer> timers = new List<Timer>();
    public static List<Timer> activetimers = new List<Timer>();
    public UInt64 Limit;
    public UInt64 timing;
    public bool active;
    public bool loop;
    public Timer(UInt64 milliseconds,bool enableoncreate, bool looping)
    {
        Limit = milliseconds;
        loop = looping;
        active = true;
        timers.Add(this);
        if (enableoncreate) activetimers.Add(this);
    }
    public void activate()
    {
        if (!active) 
        {
            activetimers.Add(this);
            active = true; 
        }
    }
    public void disable()
    {
        if (active) 
        {
            activetimers.Remove(this);
            active = false; 
        }
    }
    public void delete()
    {
        if (activetimers.Contains(this)) activetimers.Remove(this);
        if (timers.Contains(this)) timers.Remove(this);
    }
    public void reset()
    {
        timing = 0;
    }
    public void change(UInt64 milliseconds, bool looping)
    {
        Limit = milliseconds;
        loop = looping;
        timing = 0;
    }
    public event EventHandler beep;
    public void update(Time dt)
    {
        if (timing + (ulong)dt.AsMilliseconds() < Limit) // huge optimisation could be done there
        {
            timing += (ulong)dt.AsMilliseconds();
        }
        else /*if (timing + (ulong)dt.AsMilliseconds() > Limit || timing + (ulong)dt.AsMilliseconds() == Limit)*/
        {
            if (loop)
            {
                timing = 0;
                beep(this, EventArgs.Empty);
            }
            else
            {
                disable();
                beep(this, EventArgs.Empty);
            }
        }
    }
}
class AnimatedTexture
{
    public Texture[] txlist;
    public string ActiveTextureSet;
    public int frames;
    public uint[] durations; // in milliseconds
    public int index; //txlist[index].duration, what
    public Dictionary<string, AnimatedTexture> MatchDict = new Dictionary<string, AnimatedTexture>();
    public AnimatedTexture(Texture[] texture, uint[] durs)
    {
        if (durs.Length < texture.Length)
        {
            throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
        }
        txlist = texture;
        index = -1;
        frames = txlist.Length;
    }
    public uint GetDuration() //current
    {
        if(index == -1)
        {
            return durations[index + 1];
        }
        else
        {
            return durations[index];
        }
    }
    public AnimatedTexture(Texture texture, IntRect[] areas, uint[][] durs ,string[] names, uint XStep)
    {                         
        if (areas.Length != names.Length
            || areas.Length != durs.Length) throw new ArraySizesDontMatchException("Count of Elements in all given array arguments dont match");
        for(int h = 0; h < areas.Length; h++)   
        {                                      
            uint _TEMP = (uint)areas[h].Width / XStep;
            if(durs[h].Length < _TEMP)
            {
                throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
            }

            var copytoimage = texture.CopyToImage();
            Texture _TOUSEINDICT = new Texture(copytoimage, areas[h]);
            var USEINDICT = _TOUSEINDICT.CopyToImage();
            Texture[] _TOADDTODICT = new Texture[_TEMP];
            for(int LL = 0; LL < _TEMP; LL++)
            {
                _TOADDTODICT[LL] = new Texture(USEINDICT, new IntRect((int)(LL * XStep), 0, (int)XStep, (int)copytoimage.Size.Y));
            }
            var K = new AnimatedTexture(_TOADDTODICT, durs[h]);
            K.durations = durs[h];
            MatchDict.Add(names[h], K);
        }
        SetActive(names[0]);
    }
    public AnimatedTexture(Texture texture, IntRect area, uint[] durs, string names, uint XStep)
    {
        uint _TEMP = (uint)area.Width / XStep;
        if (durs.Length < _TEMP)
        {
            throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
        }

        var copytoimage = texture.CopyToImage();
        Texture _TOUSEINDICT = new Texture(copytoimage, area);
        var USEINDICT = _TOUSEINDICT.CopyToImage();
        Texture[] _TOADDTODICT = new Texture[_TEMP];
        for (int LL = 0; LL < _TEMP; LL++)
        {
            _TOADDTODICT[LL] = new Texture(USEINDICT, new IntRect((int)(LL * XStep), 0, (int)XStep, (int)copytoimage.Size.Y));
        }
        var K = new AnimatedTexture(_TOADDTODICT, durs);
        K.durations = durs;
        MatchDict.Add(names, K);
        SetActive(names);
    }
    public void AddTexture(Texture texture, IntRect area, uint[] durs,string names, uint XStep)
    {
        uint _TEMP = (uint)area.Width / XStep;
        if (durs.Length < _TEMP)
        {
            throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
        }
        var copytoimage = texture.CopyToImage();
        Texture _TOUSEINDICT = new Texture(copytoimage, area);
        var USEINDICT = _TOUSEINDICT.CopyToImage();
        Texture[] _TOADDTODICT = new Texture[_TEMP];
        for (int LL = 0; LL < _TEMP; LL++)
        {
            _TOADDTODICT[LL] = new Texture(USEINDICT, new IntRect((int)(LL * XStep), 0, (int)XStep, (int)copytoimage.Size.Y));
        }
        var K = new AnimatedTexture(_TOADDTODICT, durs);
        K.durations = durs;
        MatchDict.Add(names, K);
    }
    public void SetActive(string val)
    {
        if(MatchDict.ContainsKey(val))
        {
            ActiveTextureSet = val;
            index = -1;
            frames = MatchDict[val].frames;
            txlist = MatchDict[val].txlist;
            durations = MatchDict[val].durations;
        }
        else
        {
            throw new ArgumentException("inputted argument doesn't exist as a key in MatchDict", "val");
        }
    }
    public Texture NextTexture()
    {
        if(index + 1 <= frames - 1)
        {
            return txlist[++index];
        }
        else if(index +1 > frames - 1)
        {
            index = -1;
            return txlist[++index];
        }
        return null;
    }
}

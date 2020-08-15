using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using SFML.Window;

class Timer
{
    public static List<Timer> Timers = new List<Timer>();
    public static List<Timer> ActiveTimers = new List<Timer>();
    public UInt64 limit;
    public UInt64 timing;
    public bool active;
    public bool loop;
    public Timer(UInt64 milliseconds, bool enableOnCreate, bool looping)
    {
        limit = milliseconds;
        loop = looping;
        active = true;
        Timers.Add(this);
        if (enableOnCreate) ActiveTimers.Add(this);
    }
    public void activate()
    {
        if (!active) 
        {
            ActiveTimers.Add(this);
            active = true; 
        }
    }
    public void disable()
    {
        if (active) 
        {
            ActiveTimers.Remove(this);
            active = false; 
        }
    }
    public void delete()
    {
        if (ActiveTimers.Contains(this)) ActiveTimers.Remove(this);
        if (Timers.Contains(this)) Timers.Remove(this);
    }
    public void reset()
    {
        timing = 0;
    }
    public void change(UInt64 milliseconds, bool looping)
    {
        limit = milliseconds;
        loop = looping;
        timing = 0;
    }
    public event EventHandler Beep;
    public void update(Time dt)
    {
        if (timing + (ulong)dt.AsMilliseconds() < limit) // might polish this place a bit but not sure
        {
            timing += (ulong)dt.AsMilliseconds();
        }
        else /*if (timing + (ulong)dt.AsMilliseconds() > Limit || timing + (ulong)dt.AsMilliseconds() == Limit)*/
        {
            if (loop)
            {
                timing = 0;
                Beep(this, EventArgs.Empty);              
            }
            else
            {
                disable();
                Beep(this, EventArgs.Empty);
            }
        }
    }
}
class AnimatedTexture
{
    public Texture[] textureList;
    public string activeTextureSet;
    public int frames;
    public uint[] durations; // in milliseconds
    public int index;
    public Dictionary<string, AnimatedTexture> MatchDict = new Dictionary<string, AnimatedTexture>();
    public AnimatedTexture(Texture[] texture, uint[] durs)
    {
        if (durs.Length < texture.Length)
        {
            throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
        }
        durations = durs;
        textureList = texture;
        index = -1;
        frames = textureList.Length;
    }
    public uint GetDuration() //current
    {
        if(index == -1)
        {
            return durations[0];
        }
        return durations[index];
    }
    public AnimatedTexture(Texture texture, IntRect[] areas, uint[][] durs ,string[] names, uint XStep)
    {
        if (areas.Length != names.Length || areas.Length != durs.Length)
            { throw new ArraySizesDontMatchException("Count of Elements in all given array arguments dont match"); }
        for(int h = 0; h < areas.Length; h++)
        {
            uint frameTemp = (uint)areas[h].Width / XStep;
            if(durs[h].Length < frameTemp)
                { throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs"); }

            var copyToImage = texture.CopyToImage();
            Texture toUseInDictionary = new Texture(copyToImage, areas[h]);
            var useInDictionary = toUseInDictionary.CopyToImage();
            Texture[] toAddToDictionary = new Texture[frameTemp];
            for(int iter = 0; iter < frameTemp; iter++)
            {
                toAddToDictionary[iter] = new Texture(useInDictionary, new IntRect((int)(iter * XStep), 0, (int)XStep, (int)copyToImage.Size.Y));
            }
            MatchDict.Add(names[h], new AnimatedTexture(toAddToDictionary, durs[h]));
        }
        SetActive(names[0]);
    }
    public AnimatedTexture(Texture texture, IntRect area, uint[] durs, string names, uint XStep)
    {
        uint frameTemp = (uint)area.Width / XStep;
        if (durs.Length < frameTemp)
        {
            throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
        }

        var copyToImage = texture.CopyToImage();
        Texture toUseInDictionary = new Texture(copyToImage, area);
        var useInDictionary = toUseInDictionary.CopyToImage();
        Texture[] toAddToDictionary = new Texture[frameTemp];
        for (int iter = 0; iter < frameTemp; iter++)
        {
            toAddToDictionary[iter] = new Texture(useInDictionary, new IntRect((int)(iter * XStep), 0, (int)XStep, (int)copyToImage.Size.Y));
        }
        MatchDict.Add(names, new AnimatedTexture(toAddToDictionary, durs));
        SetActive(names);
    }
    public void AddTexture(Texture texture, IntRect area, uint[] durs,string names, uint XStep)
    {
        uint frameTemp = (uint)area.Width / XStep;
        if (durs.Length < frameTemp)
        {
            throw new ArraySizesDontMatchException("there are less elements than there are frames in given argument uint[][] durs");
        }
        var copyToImage = texture.CopyToImage();
        Texture toUseInDictionary = new Texture(copyToImage, area);
        var useInDictionary = toUseInDictionary.CopyToImage();
        Texture[] toAddToDictionary = new Texture[frameTemp];
        for (int iter = 0; iter < frameTemp; iter++)
        {
            toAddToDictionary[iter] = new Texture(useInDictionary, new IntRect((int)(iter * XStep), 0, (int)XStep, (int)copyToImage.Size.Y));
        }
        MatchDict.Add(names, new AnimatedTexture(toAddToDictionary, durs));
    }
    public void SetActive(string keyValue)
    {
        if(MatchDict.ContainsKey(keyValue))
        {
            activeTextureSet = keyValue;
            index = -1;
            frames = MatchDict[keyValue].frames;
            textureList = MatchDict[keyValue].textureList;
            durations = MatchDict[keyValue].durations;
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
            return textureList[++index];
        }
        else if(index +1 > frames - 1)
        {
            index = -1;
            return textureList[++index];
        }
        return null;
    }
}

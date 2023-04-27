using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager : Singleton<SpriteManager>
{
    static SpriteAtlas spriteAtlas;
    static Sprite[] sprites;

    public SpriteManager()
    {
        Debug.Log("SpriteManager √ ±‚»≠");
    }

    public static void Init()
    {
        spriteAtlas = Resources.Load<SpriteAtlas>("Sprites/CardSprites");
        sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);
    }

    public static Sprite Get(int i)
    {
        return sprites[i];
    }
}

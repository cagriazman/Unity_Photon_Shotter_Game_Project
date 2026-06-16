public class AK_Silah : SilahTaban
{
    protected override void AtesAnimasyonuOynat()
    {
        int upperLayer = anim.GetLayerIndex("UpperBody");
        anim.CrossFadeInFixedTime("Rig_AK_Shot", 0.02f, upperLayer, 0f);
    }

    
}
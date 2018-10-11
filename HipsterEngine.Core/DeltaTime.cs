namespace HipsterEngine.Core
{
    public class DeltaTime
    {
        private long lastTime = System.Environment.TickCount;
        private int fps = 1;
        private int frames;

        private float deltaTime = 0.005f;

        public void Update()
        {
            var currentTick = System.Environment.TickCount;
            if(currentTick - lastTime >= 1000)
            {
                fps = frames;
                frames = 0;
                deltaTime = currentTick  - lastTime;
                lastTime = currentTick;
            }
            frames++;


        }

        public int GetFPS()
        {
            return fps;
        }

        public float GetDeltaTime()
        {
            return (deltaTime / 1000.0f);
        } 
    }
}
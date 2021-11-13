# TimeJet
<img src="TimeJetECS-A.gif" alt="gif" width="1280"/>
<img src="TimeJetECS-B.gif" alt="gif" width="1280"/>

I explored how to use the Unity ECS. I ended up with an experimental prototype where you can fly around in a spaceship, where the time goes faster according to how much you use your thrusters to accelerate. Your positional and rotational velocities are independent of this time distortion. (I.e. you can give a brief thrust and then stop using your thrusters to continue moving but freezing the movement of bullets and other ships.)

I have also implemented the [Winkel tripel projection](https://www.wikiwand.com/en/Winkel_tripel_projection) in a shader, to create a 360-degree camera by studying the source code of [Blinky](https://github.com/shaunlebron/blinky).

I developed this in 5 days or so in 2019.

All of the art assets are from the Unity asset store, except the particle system for the bullets and the GUI elements.

[Click here to see a longer video.](https://youtu.be/aHQeb8UERN8)

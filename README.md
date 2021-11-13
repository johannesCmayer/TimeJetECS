# TimeJet
<img src="TimeJetECS-A.gif" alt="gif" width="1280"/>
<img src="TimeJetECS-B.gif" alt="gif" width="1280"/>

I explored how to use the Unity ECS. I ended up with an experimental prototype where you can fly around in space with a 360-degree camera. Also, time moves faster according to how much you use your thrusters to accelerate (including rotational thrusters). Your positional and rotational velocities are independent of this time distortion. This means you can give a brief thrust and then stop using your thrusters to continue moving, but freezing the movement of bullets and other ships.

To create the 360-degree camera I have implemented the [Winkel tripel projection](https://www.wikiwand.com/en/Winkel_tripel_projection) in a shader by studying the source code of [Blinky](https://github.com/shaunlebron/blinky).

I developed this in 7 days or so in 2019 (not having used ECS before).

All of the art assets are from the Unity asset store, except the particle system for the bullets and the GUI elements.

[Click here to see a longer video.](https://youtu.be/aHQeb8UERN8)

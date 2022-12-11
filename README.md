# PathFollowingCar
Given a racetrack that was printed on paper, I measured each line and curve to create a 1 for 1, to-scale replica of the track in Desmos using piecewise functions.
Racetrack Picture: ![image](https://user-images.githubusercontent.com/55565946/206883166-84b480f3-90d2-4561-bd39-b16742129efb.png)
Piecewise Funcctions: ![image](https://user-images.githubusercontent.com/55565946/206883175-650182bc-10e8-4b13-a08d-d0ad042ded38.png)

The car that I used has two wheels that cannot rotate; you turn the car by only adjusting the individual speeds of the two wheels. The physics equations relating angle and position to left wheel velocity, right wheel velocity, wheel radius, and distance between the wheels are as follows: ![image](https://user-images.githubusercontent.com/55565946/206883215-92d7e80a-5f26-48e0-9e09-3fde33605f3d.png)

To implement the racetrack in C#, I coded all of the piecewise functions and set up a for-loop which added a waypoint along every millimeter in the piecewise function to a list. The track was about 368 cm long, with curves, and there were about 4300 waypoints. To find the distance between the car and the track, I compared the car's current vector position to each vector of all of the waypoints in the list (not ideal I know).

I used an neural network which had 2 inputs (distance and velocity), 2 outputs (left wheel speed and right wheel speed). The first hidden layer had 4 neurons and the second hidden layer had 3. I used a genetic algorithm for the reinforcement learning with 50% exploration and 50% exploitation. The learning rate was gradually decreased with more iterations.

After I had obtained a car that worked theoretically on the simulator, I had to get the code to work on the actual car. I used another neural network that took in 8 sensor inputs and had 1 output which was the distance from the track. For this neural network, supervised learning was used, so I manually created a data set with about 80 values. The neural network had neurons in each layer as follows: 8, 8, 4, 2, 1. I used this distance to calculate velocity as well. In the Arduino code for the car, I tried implementing the same feedforward algorithm that I used in C#. However, with because I was unable to use classes or 2d arrays of variable size, I had to just create a bunch of arrays for the weights and biases, then create a new for loop for each layer of a neural network (not optimal again I know).

The neural network that I used for the AI had given velocities as outputs. However, the car only accepted serial inputs of between 0-255, not velocities. To combat this, I measured a few different serial inputs and their correspoinding velocities. The velocities were recorded by measuring how long it took the car to do 1 rotation. I obtained a graph which looked like this: ![image](https://user-images.githubusercontent.com/55565946/206883567-9aa354f7-0f90-4cd7-997a-1b50988ec909.png)
This was the function that I used to fit to the points: ![image](https://user-images.githubusercontent.com/55565946/206883579-0d787cea-5778-4f90-9262-12f3d4de81f6.png)

With that done, the car was actually able to run one length of the track. I then added code which made the car do a 180 when it reached the end of the track.

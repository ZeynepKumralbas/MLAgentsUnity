# ML Agents: RL vs IL
## run:
mlagents-learn config/toTarget_config.yaml --force --run-id=toTarget_multiple

tensorboard --logdir results --port 6006

<img src="images/ToyExample1.png" alt="Example 1" width="400"/>
<br/><em>Figure 1: Example 1: Agent (sphere) learns to reach the target (cube) using RL and IL.</em>

<img src="images/ToyExample2.png" alt="Example 2" width="400"/>
<br/><em>Figure 2: Example 2: Agent (sphere) learns to reach the target (cube) with obstacles using RL and IL.</em>

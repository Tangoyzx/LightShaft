# LightShaft
Shader X3 8.1

2017.5.27
完成了核心的效果。

剩下一个问题就是由于使用sampling planes来实现，所以会有带状效果，解决这个问题最好当然是加个模糊
但是该如何只对light shaft的效果进行模糊呢？

1、现在的做法是纯粹做两个相机，然后light shaft相机的深度较大，所以在后面渲染，但是这种时候只能dont clear，所以假如对light shaft相机进行模糊的话，会把原场景的东西也模糊了
2、light shaft相机使用render target，这样的话好像没办法使用到原场景的depthBuffer，没办法让原场景的物体遮挡住sampling planes
纠结中
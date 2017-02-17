# HotRunner

Solidworks二次开发

开发环境：SolidWorks2012 + VS2010

## 心得

1. 在Page OnClosing 事件中操作SW导致崩溃：
	你这个崩溃是属性页被重复关闭引起的，你可以把代码移到AfterClose事件里应该就不会有问题了，或者你也可以不改之前的代码但在把创建属性页时增加一个lockpage属性，应该也不会有问题，你这个是因为创拉伸特征的时候会自动关闭没有lockpage的而且处于打开状态的属性页。你放在onclosing里，这个属性页就会被关闭两遍，然后就崩溃了

	总之，在各种ing事件中不要操作sw


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public partial class FISceneLogo : CLSceneContext {
	[CLContextBindAttrib]
	static void Bind(DiContainer container){
		container.Bind<FIClientDataProvider>().AsSingle();
		container.Bind<FILoadingView>().FromResolveGetter<CLContextManager>(x=>{
			return x.CreateContext<FILoadingView>();
		}).AsSingle();
		container.Bind<FITransitionView>().FromResolveGetter<CLContextManager>(x=>{
			return x.CreateContext<FITransitionView>();
		}).AsSingle();
		container.Bind<FIPopupDialog>().FromResolveGetter<CLContextManager>(x=>{
			return x.CreateContext<FIPopupDialog>();
		}).AsSingle();
		container.Bind<FIPopupManager>().FromResolveGetter<CLContextManager>(x=>{
			return x.CreateContext<FIPopupManager>();
		}).AsSingle();
		container.Bind<FIDefaultRequest>().FromResolveGetter<CLContextManager>(x=>{
			return x.CreateContext<FIDefaultRequest>();
		}).AsSingle();
		container.Bind<FIEasy>().AsSingle();
		container.Bind<FITestEasy>().AsSingle();
	}
}
public partial class FISceneTitle : CLSceneContext {
	[CLContextBindAttrib]
	static void Bind(DiContainer container){
		container.Bind<GDManager>().AsSingle();

		//For network
//		container.Bind<FIHttp>().AsSingle();
//		container.Bind<FIHttpOptions>().AsSingle();

		//Data provider..
		container.Bind<FIRBaseDataTypeContainer>().AsSingle();
		container.Bind<FIHttp>().AsSingle();
		container.Bind<IDataProvider>().To<FIServerDataProvider>().AsSingle().When(new BindingCondition(c=>{
			var options = c.Container.Resolve<FIBuildOptions>();
			return options.useLogin == true;
		}));
		container.Bind<IDataProvider>().To<FIFakeServerDataProvider>().AsSingle().When(new BindingCondition(c=>{
			var options = c.Container.Resolve<FIBuildOptions>();
			return options.useLogin == false;
		}));
		container.Bind<IRuntimeData>().FromResolveGetter<IDataProvider>(x=>x.RuntimeData).AsSingle();
	}
}
public partial class FISceneGamePlay : CLSceneContext {
	[CLContextBindAttrib]
	static void Bind(DiContainer container){
		
	}
}

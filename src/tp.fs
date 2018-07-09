module TP

open System
open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation
open ProviderImplementation.ProvidedTypes
open FSharp.Quotations

let ns = "TestTP"

[<TypeProviderAssembly("stephen_tp_test")>]
do()


[<TypeProvider>]
type TestProvider(cfg: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(cfg)
    
    // make a simple type that has a field, and in that types constructor attempt to set that field with `Expr.FieldSet`
    let asm = Assembly.LoadFrom cfg.RuntimeAssembly
    
    let simpleTy = 
        let simpleTy = ProvidedTypeDefinition(asm, ns, "SimpleType", None, isErased = false)
        let field = ProvidedField("testField", typeof<bool>)
        simpleTy.AddMember field
        let boolParam = ProvidedParameter("boolValue", typeof<bool>)
        let con = ProvidedConstructor([boolParam], (fun [thisExpr; boolExpr] -> 
            Expr.FieldSet(field, boolExpr)
        ))
        simpleTy.AddMember con
        simpleTy

    let simpleProvTy = 
        let simpleProvTy = ProvidedTypeDefinition(asm, ns, "TestProvider", None, isErased = false)        
        simpleProvTy.AddMember simpleTy
        simpleProvTy
    
    do    
        this.AddNamespace(ns, [simpleProvTy])
    
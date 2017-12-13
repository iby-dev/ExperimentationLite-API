# ExperimentationLite-API
A fork of the Experimentation-Api that uses LiteDb instead of MongoDb - making the experimentation api a proper stateful self enclosed microservice. 

A proper write up will be added soon but for now this new api functions and behaves exactly the same as the Experimentation-Api. There are no behavioural or functional differences between the two different implementations. I guess the main would be that the LiteDb driver does not provide async support, where as the MongoDb driver does. Other than the two implementations are virtual indentical. This fork of the Experimentation-Api however has one crucial difference and edge over the Experiementation-Api and that is you do not need to set-up any Dbs to get it up and running. It compiles and runs straight out of the box. Leaving you the challenge of deploying it somewhere for you to consume it somehow.

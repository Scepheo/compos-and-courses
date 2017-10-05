# Sokoban course

## Included features

| Version | Feature                                       | Discuss | Where                                              |
|---------|-----------------------------------------------|---------|----------------------------------------------------|
| 6       | `nameof` Expressions                          | Do      | Level.cs, ctor                                     |
| 6       | String Interpolation                          | Do      | EntityBase.cs, GetSprite                           |
| 6       | Read-only Auto-properties                     | Do      | Level.cs, Height/Width                             |
| 6       | Auto-Property Initializers                    | Do      | EntityBase and overrides                           |
| 6       | Expression-bodied function members            | Do      | All over the place                                 |
| 6       | Null - conditional operators                  | Do      | EntityBase.cs, TriggerOverlap and TriggerUnoverlap |
| 6       | Exception filters                             | Bonus   | N/A                                                |
| 6       | index initializers                            | Bonus   | Data.cs, CharacterItemMap                          |
| 6       | `using static`                                | Bonus   | N/A                                                |
| 6       | Extension methods for collection initializers | Bonus   | N/A                                                |
| 7       | Tuples                                        | Do      | Level.cs, GetEntities                              |
| 7       | Pattern Matching                              | Do      | EntityFire.cs, CheckDestroy                        |
| 7       | `out` variables                               | Do      | Level.cs, GetEntities                              |
| 7       | Discards                                      | Bonus   | N/A                                                |
| 7       | Local Functions                               | Do      | Level.cs, GetEntities                              |
| 7       | `throw` Expressions                           | Do      | LevelLoad.cs, GetItem                              |
| 7       | More expression-bodied members                | Bonus   | N/A                                                |
| 7       | Numeric literal syntax improvements           | Bonus   | N/A                                                |
| 7.1     | default literal expressions                   | Bonus   | N/A                                                |
| 7.1     | Inferred tuple element names                  | Bonus   | N/A                                                |

## TODO

For the course:

* Seperate rendering and other "implementation details" out from user code
* Decide order of features
* Write out course
* Create starting solution

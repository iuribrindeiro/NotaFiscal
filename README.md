# F# NotaFiscal App

After learning a little bit of [Elm](https://elm-lang.org/) I decided to go a little bit deeper into what functional programming would look like, particularly in the .NET ecosystem.

I have always been a lover of C# and all its strongly typed capabilities. Learning functional has been a blast for me since it is even more strongly typed and at the same type less verbose than C#/Java or other OO languages I was used to. The biggest 2 advantages of functional for me are:

- Union Types. This one is fantastic. The idea that I don't have to deal with exceptions because I can always return something "inside a box" and only retrieve it if I make sure that I'm also covering the failure scenario for that particular path is just... amazing. Not to mention the way I can model my domain with no _`bool` flags_. I can just build specific types for each particular possible combination of state and make it so close to the real Domain language. Of course, this can be achieved with OO as well, but with Union types (more known in functional languages) this is way more natural.
- No nulls/No NullReferenceExceptions. This is kind of because of Unions as well, I guess. Using the same "box concept" we can specify something like Option<T>, like F# does. The difference is that I'll only be able to reach out to this something once I make sure that there is a value there. Otherwise, I get a compiler error. (I'm calling it "box concept" because no one knows what monads are, so I'm going to just say "box" until I learn it and I can finally say: "a monad is just a monoid in the category of endofunctors")

The goal of this repo was to experiment functional using an old challenge from a company I worked a few years ago: Create a hub that communicates between different city halls webservices around Brazil. The biggest challenge for this application was to design an interface that could interact between all these city halls in a elegant and easy way for whoever is using it. Independent of the City you are trying to integrate with. If you don't know any Portuguese, it might get hard to understand a few names here, but the core of what I learned by playing with this are here [Rop](https://github.com/iuribrindeiro/NotaFiscal/blob/master/NotaFiscal.Domain/Rop.fs) and here [NotaFiscalPrimitives](https://github.com/iuribrindeiro/NotaFiscal/blob/master/NotaFiscal.Domain/NotaFiscalPrimitives.fs).

So far, I was able to:

- Implement some crucial validations to save the principal (and so far, the only) entity from the app, a "NotaFiscal"
- Implement a little bit of the DB communication. This was tricky for me. I'm used to play with ORMs. I know that people criticize Entity Framework from C# a lot, but it makes your life _waaay_ easier if implemented correctly. (Also, it is not as slow as people say, they just don't know how to use it correctly. Which I think might be a glitch on the framework side, to be honest.).
  In F# there is no such thing as a ORM as it is in C# with EF or NHibernate. It is kind of a pain to map your Domain to tables(if working with SQL). Using the C# MongoDB provider was the approach that was sounding more simple to me. I don't have to care about mappings and I can share 1 denormalized DTO between the database and the Web, or a Job/Worker layer that might come in the future.

A few things that I thought was disappointing in F#:

- **No Exceptions? Kind of**. Since F# lives under the .NET umbrella, we still get exceptions. Even that your application doesn't throw exceptions, you still have to deal with libraries, such as: database calls, json deserialization, external communications in general, etc.
- **Option<T> sometimes might be Some(null), which is hilarious**. Again, because F# depends on a lot of C# stuff, we can still get null values in runtime. This can throw NullReference exceptions and obviously requires null checks.

Even with the above points, I still loved to play with F#. A lot of basic functional stuff, like functional composition and curried functions(<3) is something that F# does perfectly. Also, the custom operators are really powerful combined with all of that.

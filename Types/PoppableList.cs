// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Text.Json;
// using System.Text.Json.Serialization;

// namespace FOSSGames
// {
//     public class PoppableList<T> : List<T>
//     {
//         public PoppableList() : base()
//         {

//         }
//         public T PopFirst()
//         {
//             T item = this[0];
//             Remove(item);
//             return item;
//         }
//         public T Pop()
//         {
//             T item = this[Count - 1];
//             Remove(item);
//             return item;
//         }

//         public T PopLast() => Pop();
//     }
// }
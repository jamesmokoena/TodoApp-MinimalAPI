using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//add the app
builder.Services.AddSingleton<ItemRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//END-POINTS 

var items = app.MapGroup("/items");
//end-point to get all the items, I will then use the GetAll operator.
items.MapGet("/", ([FromServices] ItemRepository items) =>{
    return items.GetAll();
});

// end-point to items by id.
items.MapGet("/{id}", ([FromServices] ItemRepository items, int id) =>{

    return items.GetById(id);
});

//End-point to post a new item.
items.MapPost("/{id}", ([FromServices] ItemRepository items, Item item) => {
    //first i check if the items exists in the list already. if not then i add it, else i return a BadRequest error message.
    if (items.GetById(item.id) == null){
        items.Add (item);
        return  Results.Created($"/items/{item.id}", item);
    }
    return Results.BadRequest();
});

//End-point to update an existing item.
items.MapPut("/{id}", ([FromServices] ItemRepository items, Item item) => {
    if (items.GetById(item.id) == null){
        return  Results.BadRequest();
    }
    items.Update(item);
    return Results.NoContent();
});

//End-point to delete an existing item.
items.MapDelete("/{id}", ([FromServices] ItemRepository items, int id) => {
    if (items.GetById(id) == null){
        return  Results.BadRequest();
    }
    items.Delete(id);
    return Results.NoContent();
});


app.UseHttpsRedirection();

app.Run();

// A record to initialize the item object by this parameters.
record Item(int id, string title, bool completed);

class ItemRepository 
{
  //Dictionary to add items into, this is to act as an in-memory Database.
  public readonly Dictionary<int, Item> _items = new Dictionary<int, Item>();

   //contructor to buld an object of the items.
  public ItemRepository()
  {
       var item1 = new Item(1, "Wash the car", false);
       var item2 = new Item(2, "get groceries", false);
       var item3 = new Item(3, "cook dinner", false);
       var item4 = new Item(4, "eat dinner", false);
       var item5 = new Item(5, "read a book", false);
       var item6 = new Item(6, "go to sleep", false);

        //Adding items to the dictionary, so that when I initiaze the ocnstructor always create an object with this items
       _items.Add(item1.id, item1);
       _items.Add(item2.id, item2);
       _items.Add(item3.id, item3);
       _items.Add(item4.id, item4);
       _items.Add(item5.id, item5);
       _items.Add(item6.id, item6);




  }

  //operations which i will use to do things to do the in-memory database.
   public List<Item>  GetAll() => _items.Values.ToList();
  

   //get item by id, if it is not there return null
   public Item? GetById(int id) =>  _items.ContainsKey(id) ? _items[id] : null;
   // its like this
   //  public Item? GetById(int id) => {
   //   var results = _items.containsKey(id);
   //       if(result) 
   //         return _items[id];
   //           return null }


   public void  Add(Item obj) => _items.Add(obj.id, obj);
   public void Update(Item obj) => _items[obj.id]=obj;
   public void Delete(int id) => _items.Remove(id);




}


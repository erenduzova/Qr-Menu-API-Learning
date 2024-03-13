using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qr_Menu_API.Models
{
	public class State
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte Id { get; set; }
		[StringLength(10)] // En fazla 10 karakter
		[Column(TypeName = "nvarchar(10)")]
		public string Name { get; set; } = "";

        public State(byte id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}


<template>

<div>
  <div v-if="book">
    <div v-if="editMode === false">
      <b-card
        :title="book.title"
        :img-src="book.thumbnailUrl"
        img-alt="Image"
        img-top
        tag="article"
        style="max-width: 30rem"
        class="mb-2"
      >
        <b-card-text>
          {{ book.descr }}
        </b-card-text>
        <b-button to="/" variant="primary">Back</b-button>
        <b-button style="margin: 20px" @click="editbook" variant="primary"
          >Edit</b-button>
      </b-card>
    </div>

  </div>
     <div v-if="(editMode === true && book) || addMode === true">
      <label style="margin-top:10px;" for="booktitle">Book Title:</label>

      <b-form-textarea
        id="booktitle"
        v-model="booktitle"
        placeholder="Book Title"
        rows="2"
        max-rows="6"
      ></b-form-textarea>

         <label  style="margin-top:10px;" for="bookDesc">Book Description:</label>

             <b-form-textarea
        id="bookDesc"
        v-model="bookDesc"
        placeholder="Book Descreption"
        rows="3"
        max-rows="6"
      ></b-form-textarea>

        <label style="margin-top:10px;" for="publishedDate">Published Date</label>
    <b-form-datepicker id="publishedDate" v-model="publishedDate" class="mb-2"></b-form-datepicker>
         <label  v-if="addMode === true"  for="bookisbn">Book ISBN:</label>

             <b-form-textarea  v-if="addMode === true" 
        id="bookisbn"
        v-model="bookisbn"
        placeholder="Book ISBN"
        rows="3"
        max-rows="6"
      ></b-form-textarea>

    <b-button v-if="editMode === true" style="margin: 20px" @click="editbook" variant="primary">Cancle</b-button>    
     <b-button to="/"  v-if="addMode === true"  variant="primary">Cancle</b-button>
     <b-button   v-if="editMode === true" style="margin: 20px" @click="updatebook" variant="primary">Save</b-button>
     <b-button  v-if="addMode === true" style="margin: 20px" @click="addbook"  variant="primary">Add</b-button>

    
    </div>
</div>
</template>

<script>
import axios from "axios";

export default {
  name: "Book",
  props: ["id"],
  data: () => ({
    book: null,
    editMode: false,
    booktitle: "",
    bookDesc: "",
    bookisbn: "",
    publishedDate: "",
    addMode : false
  }),
  mounted() {
      if(this.id){
        axios.get(`https://localhost:5001/api/books/${this.id}`).then((response) => {
      this.book = response.data;
    }, err=>{
        console.log(err);
    });
      }else {
          this.addMode = true;
      }

  },
  methods: {
    editbook: function () {
      this.editMode = !this.editMode;
      if (this.editMode) {
        this.booktitle = this.book.title;
        this.bookDesc = this.book.descr;
        this.publishedDate = this.book.publishedDate;
      }
    },
    addbook: function(){
        const isValid = this.CheckControls();
        if(!isValid){
            return;
        }
        axios.post("https://localhost:5001/api/books/AddBook", {
            title: this.booktitle,
            isbn: this.bookisbn,
            publishedDate: this.publishedDate,
            descr: this.bookDesc
        }).then(response => {
            if(response.data.isValid && response.data.isSuccessful){
                                this.$router.push({ name: 'home' });

                this.createToast("Book Added Sucesfully!!", "Book Added Sucesfully!!", "success");
            } else if(!response.data.isValid){
                this.createToast("Book ISBN aldready exists!!", "Book ISBN aldready exists!!", "warning");
            } else{
                this.createToast("Book Adding  Failed!!", "Book Adding Updated Failed!!", "failed");

            }
        }, err=>{
             this.createToast("Book Adding  Failed!!", "Book Adding Updated Failed!!", "failed");
             console.log(err);
        });
    },
    updatebook: function(){
        const isValid = this.CheckControls();
        if(!isValid){
            return;
        }
        const clonedBook =  JSON.parse(JSON.stringify(this.book));
        this.book.title = this.booktitle;
        this.book.descr = this.bookDesc;
        this.book.publishedDate = this.publishedDate;
              axios.put("https://localhost:5001/api/books", this.book)
                    .then(response => {
                        if(response.data){
                    this.createToast("Book Properties Updated Sucesfully!!", "Book Properties Updated Sucesfully!!", "success");
                     this.editMode = false;
                        }else{
                    this.createToast("Book Properties Updated Failed!!", "Book Properties Updated Failed!!", "failed");

                     this.book = clonedBook;
                     this.editMode = false;
                        }
                    }, err=>{
                    this.createToast("Book Properties Updated Failed!!", "Book Properties Updated Failed!!", "failed");
                     this.editMode = false;
                     this.book = clonedBook;
                     console.log(err);
                    });    
                    },
    
    createToast: function(subject, title, varaint){
                       this.$bvToast.toast(subject, {
                    title: title,
                    variant: varaint,
                    solid: true
                     })
    },

    CheckControls: function() {
        if(  (!this.booktitle || !this.publishedDate) && this.editMode ){
           this.createToast("Invalid Form Submission", "Title and Date Published Can't be blank", "warning");   
            return false;
        }

                if(  (!this.booktitle || !this.publishedDate || !this.bookisbn) && this.addMode ){
           this.createToast("Invalid Form Submission", "Title and Date Published and Book ISBN Can't be blank", "warning");   
            return false;
        }
        return true;
    }
  },
};
</script>
<template>
    <div class="home">
        <h1>{{ msg }}</h1>
      <b-pagination
      v-model="currentPage"
      :total-rows="rows"
      :per-page="perPage"
      aria-controls="my-table"
    ></b-pagination>
        
        <p class="mt-3">Current Page: {{ currentPage }}</p>
     
            <b-form-input
              v-model="filter"
              type="search"
              debounce="500"
              placeholder="Type to Search"
            ></b-form-input>

             <b-button style="margin: 20px" @click="addbook" variant="primary">Add Book</b-button>

        <b-table striped hover :items="dataContext" :fields="fields" :sort-by.sync="sortBy" 
            :per-page="perPage"
            :filter="filter"
            :current-page="currentPage"
            :sort-desc.sync="sortDesc"
        responsive="sm">
            <template v-slot:cell(thumbnailUrl)="data">
                <b-img :src="data.value" thumbnail fluid></b-img>
            </template>
            <template v-slot:cell(title_link)="data">
                <b-link :to="{ name: 'book_view', params: { 'id' : data.item.bookId } }">{{ data.item.title }}</b-link>
            </template>
        </b-table>
    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        name: 'Home',
        props: {
            msg: String
        },
        data: () => ({
            sortBy: 'isbn',
            sortDesc: false,
            perPage: 10,
            currentPage: 1,
            filter: null,
            rows: 0,
            fields: [
                { key: 'thumbnailUrl', label: 'Book Image' },
                { key: 'title_link', label: 'Book Title' },
                { key: 'isbn', label: 'ISBN', sortable: true },
                { key: 'descr', label: 'Description' }

            ],
            items: []
        }),
       
        methods: {
            dataContext(ctx, callback) {
                axios.post("https://localhost:5001/api/books", ctx)
                    .then(response => {
                        this.items = response.data.bookResponses;
                        this.rows = response.data.totalBooks;
                        callback(this.items);
                    }, err=>{
                    this.$bvToast.toast('Error Error!!', {
                    title: 'Something Wrong Happend, Please try again after some time',
                    variant: 'danger',
                    solid: true
                     })
                     console.log(err);
                    });
            },
            addbook(){
                            this.$router.push({ name: 'book_view' });
            }
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>


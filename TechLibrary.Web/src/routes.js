import Vue from 'vue';
import VueRouter from 'vue-router';

Vue.use(VueRouter);

const Home = () => import(/* webpackChunkName: "Home" */ './components/Home.vue');
const Book = () => import(/* webpackChunkName: "Book" */ './components/Book.vue');

const router = new VueRouter({
  routes: [
    { name:'home', path: '/', component: Home },
    {
      name: 'book_view',
      path: '/book/:id/:page',
      component: Book,
      props: true
    }
  ]
});

export default router;
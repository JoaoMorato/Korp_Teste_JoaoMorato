export default class Pagination {
    currentPage: number = 1;
    pageSize: number = 10;
    totalItems: number = 0;

    get totalPages(): number {
        return Math.ceil(this.totalItems / this.pageSize);
    }

    Next(): void {
        if (this.currentPage == this.totalPages)
            return;
        this.currentPage++;
    }

    Previus(): void {
        if (this.currentPage == 1)
            return;
        this.currentPage--;
    }
}
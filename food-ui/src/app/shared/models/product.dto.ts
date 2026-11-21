export interface ProductDto {
  id: number;
  name: string;
  type: string;
  stock: number;
  price: number | null;
  cookDate: string;
  chefId: number;
}
